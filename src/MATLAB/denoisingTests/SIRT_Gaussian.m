
for I=1:size(sinoGram,2)
    Projections{I}=sinoGram(:,I)';
end
reconImageSIRT=reconImage;

    nProjections = length(Projections);

    angleStep=360.0/nProjections;

    sizeProjections = length(Projections{1});
  
    R_Thresh = (sizeProjections/2.1)^2;
    centerI= sizeProjections/2;
    centerJ= sizeProjections/2;
    mask=zeros([sizeProjections sizeProjections]);

    for I=1:sizeProjections
            for J=1:sizeProjections
                R= ((I-centerI)^2 + (J-centerJ)^2)/R_Thresh;
                if R>1
                    mask(I,J)=1;
                end
            end
    end
    maskIndex = find(mask==1);

   addition = zeros([sizeProjections sizeProjections]);

   
   myfilter = fspecial('gaussian',[5 5], 0.5);

   
    for K=1:10
        diffImage = zeros([sizeProjections sizeProjections]);
        reconImageSIRT(maskIndex)=0;

        for I=1:nProjections
            projection = Projections{I};

            tempImage = imrotate(reconImageSIRT,-1*angleStep*I,'bilinear','crop'); 

            forward = sum(tempImage);

            diffProjections{I}=(projection -forward)/sizeProjections;
        end

        
        for I=1:nProjections
            diff = diffProjections{I}/nProjections;

            for J=1:sizeProjections
               addition(:,J)= diff(J);
            end

            addition=imrotate(addition,angleStep*I,'bilinear','crop');

            diffImage=diffImage + addition;
        end
        
        
        
        imagesc(diffImage);
        drawnow;

        an=1.0/K;

        %regularization = Poisson_denoising_Anscombe_exact_unbiased_inverse(reconImageSIRT) ;
        
        regularization = imfilter(reconImageSIRT,myfilter,'replicate');
        reconImageSIRT=(reconImageSIRT *an + (1-an)* regularization) + an* diffImage;
    end
    
    regularization = mrf(reconImageSIRT,3,300,.7,5) ;

    R_Thresh = (sizeProjections/2.2)^2;
    sumV=0;
    cc=0;
    for I=1:sizeProjections
            for J=1:sizeProjections
                R= ((I-centerI)^2 + (J-centerJ)^2)/R_Thresh;
                if R>1 && mask(I,J)~=1
                    sumV=sumV + reconImageSIRT(I,J);
                    cc=cc+1;
                end
            end
    end

    average = sumV/cc;

    reconImageSIRT(maskIndex)=average;


    figure;imagesc(reconImageSIRT(70:end-70,70:end-70));colormap gray;


