
testImages=cleanedImages2;

nProjections =length(testImages);

step=(length(testImages)-2)/nProjections;

angleStep=360.0/(nProjections+1);

s=size(testImages{2});
sizeProjections = s(2);

reconImage = zeros([sizeProjections sizeProjections]);


B=(sizeProjections/-2):(sizeProjections/2-1);
filter(1:length(B)) =abs(B);
filter=filter./(length(filter)/2)^2*3.14;
han=hann(length(filter))';
%filter=filter.*han;

filter=fftshift(filter);

s = size(testImages{2});


sinoGram = zeros([s(2) nProjections]);

addition = zeros([sizeProjections sizeProjections]);
for I=2:nProjections
    sI=round( (I-1)*step+1);
    projection = testImages{sI};
    
    projection= medfilt2(projection, [9 9]);
    
    
    projection = projection(:,round(s(1)/2))';
    
    Projections{I}=projection;
     
    sinoGram(:,I)=projection(:);
    
    fProjection =real(ifft(  fft(projection) .* filter ));
    
    
    for J=1:sizeProjections
       addition(:,J)= fProjection(J);
    end
    
    addition=imrotate(addition,angleStep*I,'nearest','crop');
    
    reconImage=reconImage + addition;
end

figure;imagesc(reconImage);colormap gray;
D=reconImage;


