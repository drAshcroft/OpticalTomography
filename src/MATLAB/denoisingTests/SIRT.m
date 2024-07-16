

nProjections = length(Projections);

angleStep=360.0/nProjections;

sizeProjections = length(Projections{40});


reconImageSIRT=reconImage;


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

for K=1:15
    diffImage = zeros([sizeProjections sizeProjections]);
    reconImageSIRT(maskIndex)=0;
    
    for I=11:nProjections
        projection = Projections{I};

        tempImage = imrotate(reconImageSIRT,-1*angleStep*I,'bilinear','crop'); 
        
        forward = sum(tempImage);
        
        diffProjections{I}=projection -forward;
    end

    for I=11:nProjections
        diff = diffProjections{I}/(sizeProjections^2);
     
        for J=1:sizeProjections
           addition(:,J)= diff(J);
        end

        addition=imrotate(addition,angleStep*I,'bilinear','crop');

        diffImage=diffImage + addition;
    end
    imagesc(diffImage);
    drawnow;
    a=1;
    if K>5
        a=1.0/(K-5);
    end
    
    reconImageSIRT=reconImageSIRT + a* diffImage;
end

figure;imagesc(reconImageSIRT(70:end-70,70:end-70));colormap gray;


double(max(D(:)))/max(reconImageSIRT(:))